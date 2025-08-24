import ReinviteModal from "@/pages/event/components/reinvite";
import { apiAddLead, apiChangeTele, apiDeleteLead, apiUpdateLead, apiUpdateLeadPhoneNumber, apiUpdateLeadStatus, listLead } from "@/services/contact";
import { apiSmOptions, apiTeleWithTmOptions } from "@/services/user";
import { BranchesOutlined, DeleteOutlined, EditOutlined, ManOutlined, MoreOutlined, PhoneOutlined, PlusOutlined, SettingOutlined, UserOutlined, WomanOutlined } from "@ant-design/icons";
import { ActionType, ModalForm, PageContainer, ProFormDatePicker, ProFormInstance, ProFormSelect, ProFormText, ProFormTextArea, ProTable } from "@ant-design/pro-components"
import { useAccess, useIntl, useModel } from "@umijs/max";
import { Button, Col, Dropdown, message, Popconfirm, Row, Tag } from "antd";
import dayjs from "dayjs";
import { useEffect, useRef, useState } from "react";
import UpdateBranchModal from "./components/update-branch";
import { BRANCH_OPTIONS } from "@/utils/constants";

const LeadPage: React.FC = () => {

    const actionRef = useRef<ActionType>();
    const [open, setOpen] = useState<boolean>(false);
    const [openBranch, setOpenBranch] = useState<boolean>(false);
    const [lead, setLead] = useState<any>();
    const access = useAccess();
    const formRef = useRef<ProFormInstance>();
    const { initialState } = useModel('@@initialState');
    const [total, setTotal] = useState<number>(0);
    const intl = useIntl();
    const [smOptions, setSmOptions] = useState<any>([]);
    const [teleOptions, setTeleOptions] = useState<any[]>([]);
    const [openHistory, setOpenHistory] = useState<boolean>(false);
    const [openPhone, setOpenPhone] = useState<boolean>(false);
    const [openTele, setOpenTele] = useState<boolean>(false);

    useEffect(() => {
        apiSmOptions().then(response => setSmOptions(response));
        apiTeleWithTmOptions().then(response => setTeleOptions(response));
        if (lead) {
            formRef.current?.setFields([
                {
                    name: 'id',
                    value: lead.id
                },
                {
                    name: 'name',
                    value: lead.name
                },
                {
                    name: 'phoneNumber',
                    value: lead.phoneNumber
                },
                {
                    name: 'email',
                    value: lead.email
                },
                {
                    name: 'dateOfBirth',
                    value: lead.dateOfBirth
                },
                {
                    name: 'address',
                    value: lead.address
                },
                {
                    name: 'eventTime',
                    value: lead.eventTime
                },
                {
                    name: 'eventDate',
                    value: lead.eventDate
                },
                {
                    name: 'identityNumber',
                    value: lead.identityNumber
                },
                {
                    name: 'gender',
                    value: lead.gender
                },
                {
                    name: 'note',
                    value: lead.note
                },
                {
                    name: 'branch',
                    value: lead.branch
                },
                {
                    name: 'telesaleId',
                    value: lead.telesaleId
                }
            ])
        }
    }, [lead])

    const onFinish = async (values: any) => {
        if (values.dateOfBirth) {
            values.dateOfBirth = dayjs(values.dateOfBirth).format('YYYY-MM-DD');
        }
        if (values.eventDate) {
            values.eventDate = dayjs(values.eventDate).format('YYYY-MM-DD');
        }
        if (values.id) {
            await apiUpdateLead(values);
        } else {
            await apiAddLead(values);
        }
        message.success('Thành công!');
        actionRef.current?.reload();
        setOpen(false);
    }

    const onChangeTele = async (values: any) => {
        if (!lead) {
            message.warning('Vui lòng chọn Key-In');
            return;
        }
        await apiChangeTele(values);
        message.success('Đổi thành công!');
        actionRef.current?.reload();
        setLead(null);
        setOpenTele(false);
    }

    return (
        <PageContainer
            subTitle={`Tổng: ${intl.formatNumber(total)}`}
            extra={<Button type="primary" icon={<PlusOutlined />} onClick={() => setOpen(true)} hidden={!access.sales && !access.telesale && !access.telesaleManager}>Tạo mới</Button>}>
            <ProTable
                actionRef={actionRef}
                search={{
                    layout: 'vertical'
                }}
                scroll={{
                    x: true
                }}
                pagination={{
                    showTotal: (total) => {
                        setTotal(total)
                        return 'Tổng ' + intl.formatNumber(total);
                    }
                }}
                request={listLead}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30
                    },
                    {
                        title: 'Họ và tên',
                        dataIndex: 'name',
                        render: (dom, entity) => (
                            <div>{entity.gender === true && (<ManOutlined className='text-blue-500' />)}{entity.gender === false && (<WomanOutlined className='text-red-500' />)} {dom}</div>
                        ),
                        minWidth: 150
                    },
                    {
                        title: 'Điện thoại',
                        dataIndex: 'phoneNumber',
                        width: 100
                    },
                    {
                        title: 'Email',
                        dataIndex: 'email'
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
                        title: 'Địa chỉ',
                        dataIndex: 'address',
                        search: false,
                        minWidth: 80
                    },
                    {
                        title: 'Ngày sự kiện',
                        dataIndex: 'eventRange',
                        valueType: 'dateRange',
                        width: 100,
                        render: (_, entity) => entity.eventDate ? dayjs(entity.eventDate).format('DD-MM-YYYY') : '-',
                        minWidth: 100
                    },
                    {
                        title: 'Thời gian',
                        dataIndex: 'eventTime',
                        width: 80,
                        valueEnum: {
                            '09:00': '09:00',
                            '14:30': '14:30'
                        }
                    },
                    {
                        title: 'Trợ lý',
                        dataIndex: 'saleName',
                        search: false,
                        minWidth: 120
                    },
                    {
                        title: 'Người gọi',
                        dataIndex: 'teleName',
                        search: false,
                        minWidth: 120
                    },
                    {
                        title: 'Chi nhánh',
                        dataIndex: 'branch',
                        minWidth: 100,
                        valueType: 'select',
                        fieldProps: {
                            options: BRANCH_OPTIONS
                        }
                    },
                    {
                        title: 'Ghi chú',
                        dataIndex: 'note',
                        search: false,
                        minWidth: 100
                    },
                    {
                        title: 'Team',
                        dataIndex: 'smId',
                        hideInTable: true,
                        valueType: 'select',
                        fieldProps: {
                            options: smOptions
                        }
                    },
                    {
                        title: 'Trạng thái',
                        dataIndex: 'status',
                        valueEnum: {
                            0: <Tag color="yellow" className="w-20 text-center">Chờ duyệt</Tag>,
                            1: <Tag color="orange" className="w-20 text-center">Đã duyệt</Tag>,
                            2: <Tag color="red" className="w-20 text-center">Check-In</Tag>,
                            3: <Tag color="blue" className="w-20 text-center">Chốt deal</Tag>,
                            4: <Tag color="black" className="w-20 text-center">Từ chối</Tag>,
                            5: <Tag color="green" className="w-20 text-center">Hoàn thành</Tag>,
                            6: <Tag color="green">KT Xác nhận</Tag>,
                            7: <Tag color="green">GĐ Xác nhận</Tag>,
                            8: <Tag color="tomato">Mời lại</Tag>
                        },
                        search: false,
                        width: 100
                    },
                    {
                        title: 'Lượt',
                        dataIndex: 'inviteCount',
                        search: false,
                        render: (_, entity) => {
                            return <Button size="small" 
                            onClick={() => {
                                setLead(entity);
                                setOpenHistory(true);
                            }}
                            type="dashed" disabled={entity.inviteCount === 1}>{entity.inviteCount}</Button>
                        }
                    },
                    {
                        title: <SettingOutlined />,
                        valueType: 'option',
                        render: (_, entity) => [
                            <Button type="primary" size="small" key="edit" icon={<EditOutlined />} onClick={() => {
                                setLead(entity);
                                setOpen(true);
                            }} hidden={!access.canSales || entity.status !== 0} />,
                            <Popconfirm title="Xác nhận phê duyệt" key="approve" onConfirm={async () => {
                                await apiUpdateLeadStatus({
                                    id: entity.id,
                                    status: 1
                                });
                                message.success('Thành công');
                                actionRef.current?.reload();
                            }}>
                                <Button type="primary" size="small" hidden={(!access.canSm && !access.telesaleManager) || entity.status !== 0}>Phê duyệt</Button>
                            </Popconfirm>,
                            <Popconfirm title="Xác nhận xóa?" key="delete" onConfirm={async () => {
                                await apiDeleteLead(entity.id);
                                message.success('Thành công!');
                                actionRef.current?.reload();
                            }}>
                                <Button type="primary" danger size="small" icon={<DeleteOutlined />} hidden={!access.canAdmin} />
                            </Popconfirm>,
                            <Dropdown key="event" menu={{
                                items: [
                                    {
                                        key: 'phone',
                                        label: 'Đổi số điện thoại',
                                        icon: <PhoneOutlined />,
                                        disabled: !access.event
                                    },
                                    {
                                        key: 'tele',
                                        label: 'Đổi người gọi',
                                        icon: <UserOutlined />,
                                        disabled: !access.event
                                    },
                                    {
                                        key: 'branch',
                                        label: 'Đổi chi nhánh',
                                        icon: <BranchesOutlined />,
                                        disabled: !access.dot && !access.dos && !access.sm && !access.telesaleManager
                                    }
                                ],
                                onClick: (info) => {
                                    setLead(entity);
                                    if (info.key === 'phone') {
                                        setOpenPhone(true);
                                        return;
                                    }
                                    if (info.key === 'tele') {
                                        setOpenTele(true);
                                        return;
                                    }
                                    if (info.key === 'branch') {
                                        setOpenBranch(true);
                                        return;
                                    }
                                }
                            }}>
                                <Button icon={<MoreOutlined />} size="small" type="dashed"></Button>
                            </Dropdown>
                        ],
                        width: 30,
                        hideInTable: access.canAdmin,
                        align: 'center'
                    }
                ]}
            />
            <ModalForm open={open} title="Khách hàng tiềm năng" onFinish={onFinish} onOpenChange={setOpen} formRef={formRef}>
                <ProFormText name="id" hidden />
                <Row gutter={16}>
                    <Col xs={24} md={12}>
                        <ProFormText name="name" label="Họ và tên" rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                    <Col xs={12} md={8}>
                        <ProFormDatePicker name="dateOfBirth" label="Ngày sinh" fieldProps={{
                            format: {
                                type: 'mask',
                                format: 'DD-MM-YYYY'
                            },
                            className: 'w-full'
                        }} />
                    </Col>
                    <Col xs={12} md={4}>
                        <ProFormSelect label="Giới tính" name='gender' options={[
                            {
                                label: 'Nam',
                                value: true as any
                            },
                            {
                                label: 'Nữ',
                                value: false as any
                            },
                            {
                                label: 'Khác',
                                value: null
                            }
                        ]} />
                    </Col>
                    <Col xs={12} md={8}>
                        <ProFormText name="phoneNumber" label="Số điện thoại" rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                    <Col xs={12} md={8}>
                        <ProFormText name="email" label="Email" rules={[
                            {
                                type: 'email'
                            }
                        ]} />
                    </Col>
                    <Col xs={12} md={8}>
                        <ProFormText name="identityNumber" label="CCCD" />
                    </Col>
                    <Col xs={12} md={4}>
                        <ProFormSelect options={BRANCH_OPTIONS} name="branch" label="Chi nhánh" fieldProps={{
                            defaultValue: initialState?.currentUser?.branch
                        }} disabled={!access.telesale && !access.telesaleManager && !access.dot} />
                    </Col>
                    <Col xs={24} md={8}>
                        <ProFormText name="address" label="Địa chỉ" />
                    </Col>
                    <Col md={6} xs={12}>
                        <ProFormDatePicker name="eventDate" label="Ngày sự kiện" rules={[
                            {
                                required: true
                            }
                        ]} fieldProps={{
                            format: {
                                type: 'mask',
                                format: 'DD-MM-YYYY'
                            },
                            className: 'w-full'
                        }} />
                    </Col>
                    <Col md={6} xs={12}>
                        <ProFormSelect name="eventTime" label="Thời gian" options={[
                            {
                                label: '09:00',
                                value: '09:00'
                            },
                            {
                                label: '14:30',
                                value: '14:30'
                            }
                        ]} rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                    <Col md={24} xs={24} hidden={!access.telesaleManager}>
                        <ProFormSelect name="telesaleId" label="Người gọi" options={teleOptions} showSearch />
                    </Col>
                </Row>
                <ProFormTextArea label="Ghi chú" name="note" />
            </ModalForm>
            <ReinviteModal open={openHistory} onCancel={() => setOpenHistory(false)} leadId={lead?.id} />
            <ModalForm open={openPhone} onOpenChange={setOpenPhone} title="Đổi số điện thoại" onFinish={async (values) => {
                values.id = lead?.id;
                await apiUpdateLeadPhoneNumber(values);
                message.success('Thành công!');
                setLead(null);
                setOpenPhone(false);
                actionRef.current?.reload();
            }}>
                <ProFormText name="phoneNumber" label="Số điện thoại mới" rules={[
                    {
                        required: true
                    }
                ]} />
            </ModalForm>
            <ModalForm title={`Đổi người gọi của ${lead?.name}`} open={openTele} onOpenChange={setOpenTele} onFinish={onChangeTele}>
                <ProFormSelect name="telesaleId" label="Người gọi" options={teleOptions} rules={[
                    {
                        required: true
                    }
                ]} />
            </ModalForm>
            <UpdateBranchModal keyIn={lead} open={openBranch} onOpenChange={setOpenBranch} reload={() => actionRef.current?.reload()} />
        </PageContainer>
    )
}

export default LeadPage;