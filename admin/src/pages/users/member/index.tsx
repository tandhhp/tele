import { apiCardOptions, apiExportCardHolder, apiGetSalesOptions, apiSalesOptions, apiSetSaller, apiSmOptions, apiUpdateContractCode, apiUpdateLoyalty, deleteUser, apiUserList } from '@/services/user';
import { ArrowRightOutlined, AuditOutlined, DeleteOutlined, EditOutlined, ExportOutlined, EyeOutlined, FundOutlined, MailOutlined, ManOutlined, MoneyCollectOutlined, MoreOutlined, SendOutlined, UserAddOutlined, WomanOutlined } from '@ant-design/icons';
import {
    ActionType,
    ModalForm,
    PageContainer,
    ProColumns,
    ProFormInstance,
    ProFormSelect,
    ProFormText,
    ProTable,
} from '@ant-design/pro-components';
import { FormattedMessage, history } from '@umijs/max';
import { Button, Dropdown, message, Popconfirm, Space, Tag, Tooltip } from 'antd';
import { useEffect, useRef, useState } from 'react';
import OpenLoyalty from '../components/loyalty';
import { useAccess } from '@umijs/max';
import SendEmailComponent from '../components/send-email';
import TopUpModal from '../components/top-up';
import CardHolderForm from './components/form';
import dayjs from 'dayjs';
import { apiResendCreateHD } from '@/services/contact';
import PointDrawer from './components/point';
import { BRANCH_OPTIONS } from '@/utils/constants';
import LoanPointModal from './components/loan-point';

const CardHolderPage: React.FC = () => {
    const actionRef = useRef<ActionType>();
    const [open, setOpen] = useState<boolean>(false);
    const [user, setUser] = useState<any>();
    const [openLoyalty, setOpenLoyalty] = useState<boolean>(false);
    const access = useAccess();
    const [openSeller, setOpenSeller] = useState<boolean>(false);
    const salesForm = useRef<ProFormInstance>();
    const [openEmail, setOpenEmail] = useState<boolean>(false);
    const [openTopUp, setOpenTopUp] = useState<boolean>(false);
    const [saleOptions, setSaleOptions] = useState<any[]>([]);
    const [cardOptions, setCardOptions] = useState<any[]>([]);
    const [smOptions, setSmOptions] = useState<any[]>([]);
    const [openContract, setOpenContract] = useState<boolean>(false);
    const contractFormRef = useRef<ProFormInstance>();
    const [openPoint, setOpenPoint] = useState<boolean>(false);
    const [openLoan, setOpenLoan] = useState<boolean>(false);

    useEffect(() => {
        contractFormRef.current?.setFields([
            {
                name: 'contractCode',
                value: user?.contractCode
            }
        ])
    }, [user]);

    const onConfirm = async (id?: string) => {
        const response = await deleteUser(id);
        if (response.succeeded) {
            message.success('Deleted');
            actionRef.current?.reload();
        }
    }

    useEffect(() => {
        salesForm.current?.setFieldValue('sellerId', user?.sellerId);

        apiSalesOptions().then(response => {
            setSaleOptions(response);
        });
        apiCardOptions().then(response => {
            setCardOptions(response);
        });
        apiSmOptions().then(response => {
            setSmOptions(response);
        });

    }, [user]);

    const columns: ProColumns<any>[] = [
        {
            title: '#',
            valueType: 'indexBorder',
            width: 30,
            align: 'center'
        },
        {
            title: 'Hạng',
            dataIndex: 'cardId',
            render: (dom, entity) => <Tag color={entity.tierColor} className='w-full text-center'>{entity.tierName}</Tag>,
            width: 90,
            valueType: 'select',
            fieldProps: {
                options: cardOptions
            }
        },
        {
            title: 'Chi nhánh',
            dataIndex: 'branch',
            search: false,
            fieldProps: {
                options: BRANCH_OPTIONS
            },
            valueType: 'select',
            minWidth: 100,
            hideInTable: !access.canCXM
        },
        {
            title: 'Tài khoản',
            dataIndex: 'userName'
        },
        {
            title: 'Mã HĐ',
            dataIndex: 'contractCode',
            width: 100,
            minWidth: 100,
            render: (dom, entity) => (
                <div>{entity.contractCode}{entity.hasSubContract ? '⭐' : ''}</div>
            )
        },
        {
            title: 'Họ & tên',
            dataIndex: 'name',
            render: (dom, entity) => (
                <div>{entity.gender === true && (<ManOutlined className='text-blue-500' />)}{entity.gender === false && (<WomanOutlined className='text-red-500' />)} {dom}</div>
            )
        },
        {
            title: 'Email',
            dataIndex: 'email',
            hideInTable: true
        },
        {
            title: <FormattedMessage id='general.phoneNumber' />,
            dataIndex: 'phoneNumber',
            render: (dom, entity) => entity.phoneNumberHide,
            width: 90
        },
        {
            title: 'Ngày sinh',
            dataIndex: 'dateOfBirth',
            valueType: 'date',
            search: false,
            width: 100,
            render: (_, entity) => entity.dateOfBirth ? dayjs(entity.dateOfBirth).format('DD-MM-YYYY') : '-'
        },
        {
            title: 'Điểm NP',
            dataIndex: 'loyalty',
            search: false,
            valueType: 'digit',
            width: 80,
            render: (dom, entity) => (
                <Button type='link' size='small' onClick={() => {
                    setUser(entity);
                    setOpenPoint(true);
                }}>{dom}</Button>
            )
        },
        {
            title: 'Số tiền',
            dataIndex: 'amount',
            search: false,
            valueType: 'digit',
            width: 70
        },
        {
            title: 'Trợ lý',
            dataIndex: 'salesId',
            hideInTable: !access.canDos && !access.canAdmin && !access.canSm,
            render: (dom, entity) => (
                <div>
                    <Tooltip title="Đổi chuyên viên trợ lí cá nhân">
                        <Button type='text' size='small' icon={<EditOutlined />} onClick={() => {
                            setUser(entity);
                            setOpenSeller(true);
                        }} />
                    </Tooltip>
                    <Tooltip title={entity.saleUserName}>
                        {entity.saleName}
                    </Tooltip>
                </div>
            ),
            valueType: 'select',
            fieldProps: {
                options: saleOptions
            },
            search: access.canSm as any
        },
        {
            title: 'Quản lý',
            dataIndex: 'smId',
            hideInTable: !access.canDos && !access.canAdmin && !access.canSm,
            render: (dom, entity) => (
                <Tooltip title={entity.smUserName}>
                    {entity.smName}
                </Tooltip>
            ),
            valueType: 'select',
            fieldProps: {
                options: smOptions
            },
            search: access.canDos
        },
        {
            title: 'Tác vụ',
            valueType: 'option',
            render: (dom, entity) => [
                <Tooltip key="email">
                    <Popconfirm title="Xác nhận gửi email?" onConfirm={async () => {
                        await apiResendCreateHD(entity.id);
                        message.success('Gửi thành công!');
                    }}>
                        <Button type='primary' size='small' icon={<MailOutlined />} hidden={!access.canCX} />
                    </Popconfirm>
                </Tooltip>,
                <Dropdown key="more" menu={{
                    items: [
                        {
                            label: 'Xem chi tiết',
                            key: 'detail',
                            disabled: !access.canCardHolder,
                            icon: <EyeOutlined />
                        },
                        {
                            label: 'Sửa thông tin',
                            key: 'edit',
                            disabled: !access.canCRUDCardHolder,
                            icon: <EditOutlined />
                        },
                        {
                            label: 'Cập nhật hợp đồng',
                            key: 'updateContract',
                            icon: <AuditOutlined />,
                            disabled: !access.cx
                        },
                        {
                            label: 'Nạp điểm',
                            key: 'loyalty',
                            disabled: !access.canDeposit,
                            icon: <MoneyCollectOutlined />
                        },
                        {
                            key: 'topup',
                            label: 'Nạp tiền',
                            icon: <FundOutlined />,
                            disabled: !access.sales && !access.sm
                        },
                        {
                            key: 'loan',
                            label: 'Vay điểm',
                            icon: <ArrowRightOutlined />,
                            disabled: !access.cx && !access.sales && !access.canAdmin
                        }
                    ],
                    onClick: (info) => {
                        setUser(entity);
                        if (info.key === 'updateContract') {
                            setOpenContract(true);
                        }
                        if (info.key === 'detail') {
                            history.push(`/users/member/${entity.id}`);
                        }
                        if (info.key === 'loyalty') {
                            setOpenLoyalty(true);
                        }
                        if (info.key === 'edit') {
                            setOpen(true);
                        }
                        if (info.key === 'topup') {
                            setOpenTopUp(true);
                        }
                        if (info.key === 'loan') {
                            setOpenLoan(true);
                        }
                    }
                }}>
                    <Button type='dashed' size='small' icon={<MoreOutlined />}></Button>
                </Dropdown>,
                <Popconfirm title="Xác nhận xóa?" key={2} onConfirm={() => onConfirm(entity.id)}>
                    <Button type="primary" icon={<DeleteOutlined />} size='small' danger hidden={!access.canAdmin} />
                </Popconfirm>
            ],
            width: 60
        },
    ];

    const [loading, setLoading] = useState<boolean>(false);
    const [params1, setParams1] = useState<any>();

    const onExport = () => {
        setLoading(true);
        apiExportCardHolder(params1).then(response => {
            const url = window.URL.createObjectURL(
                new Blob([response]),
            );
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute(
                'download',
                `data-nuras.xlsx`,
            );

            // Append to html link element page
            document.body.appendChild(link);

            // Start download
            link.click();

            // Clean up and remove the link
            link.parentNode?.removeChild(link);
            setLoading(false);
        })
    }

    return (
        <PageContainer extra={
            <Space>
                <Button type='primary' icon={<UserAddOutlined />} onClick={() => setOpen(true)} hidden={!access.canCRUDCardHolder}>Tạo mới</Button>
                <Button icon={<ExportOutlined />} onClick={onExport} loading={loading} hidden={!access.canAdmin}>Xuất Excel</Button>
                <Button type='primary' icon={<SendOutlined />} onClick={() => setOpenEmail(true)} hidden={!access.canCX}>Gửi Email hàng loạt</Button>
            </Space>
        }>
            <ProTable<API.User>
                scroll={{
                    x: true
                }}
                rowKey="id"
                request={(params) => {
                    setParams1(params)
                    return apiUserList(params);
                }}
                columns={columns}
                actionRef={actionRef}
                search={{
                    layout: 'vertical',
                }}
            />
            <CardHolderForm user={user} open={open} setOpen={setOpen} actionRef={actionRef} />
            <OpenLoyalty
                title={`Nạp điểm cho: ${user?.name} - Điểm hiện tại: ${user?.loyalty}`}
                open={openLoyalty} onOpenChange={setOpenLoyalty} onFinish={async (values) => {
                    values.userId = user?.id;
                    await apiUpdateLoyalty(values);
                    message.success('Nạp điểm thành công!');
                    actionRef.current?.reload();
                    setOpenLoyalty(false);
                }} />
            <ModalForm title="Đổi chuyên viên trợ lý cá nhân" open={openSeller} onOpenChange={setOpenSeller} formRef={salesForm} onFinish={async (values: any) => {
                await apiSetSaller({
                    SellerId: values.sellerId,
                    CardHolderId: user?.id
                });
                message.success('Phân chủ thẻ thành công, đang chờ giám đốc phê duyệt!');
                actionRef.current?.reload();
                setOpenSeller(false);
            }}>
                <ProFormSelect label="Chọn chuyên viên" name="sellerId" request={apiGetSalesOptions} rules={[
                    {
                        required: true
                    }
                ]} />
            </ModalForm>
            <SendEmailComponent open={openEmail} onOpenChange={setOpenEmail} />
            <TopUpModal open={openTopUp} onOpenChange={setOpenTopUp} id={user?.id} />
            <ModalForm title="Cập nhật hợp đồng" open={openContract} onOpenChange={setOpenContract} formRef={contractFormRef} onFinish={async (values) => {
                values.userId = user?.id;
                await apiUpdateContractCode(values);
                message.success('Cập nhật hợp đồng thành công!');
                actionRef.current?.reload();
                setOpenContract(false);

            }}>
                <ProFormText label="Mã hợp đồng" name="contractCode" rules={
                    [
                        {
                            required: true
                        }
                    ]
                } />
            </ModalForm>
            <PointDrawer cardHolder={user} open={openPoint} onClose={() => setOpenPoint(false)} />
            <LoanPointModal open={openLoan} onOpenChange={setOpenLoan} cardHolder={user} />
        </PageContainer>
    );
};

export default CardHolderPage;
