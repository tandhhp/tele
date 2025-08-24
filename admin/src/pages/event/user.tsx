import { apiUpdateLeadStatus, apiUsersInEvent } from "@/services/contact";
import { BRANCH_OPTIONS, LeadStatus, SOURCES } from "@/utils/constants";
import { CheckOutlined, CloseOutlined, CommentOutlined, CopyOutlined, EditOutlined, ManOutlined, MoreOutlined, PlusOutlined, UsergroupAddOutlined, WomanOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { Button, DatePicker, Dropdown, message, Popconfirm, Space, Tag, Tooltip } from "antd";
import { useRef, useState } from "react";
import LeadFeedback from "./components/feedback";
import dayjs from "dayjs";
import { useAccess, useModel } from "@umijs/max";
import LeadForm from "./components/lead-form";
import SubLead from "./components/sublead";
import TableComponent from "./components/table";
import ReinviteModal from "./components/reinvite";
import BackToCheckinModal from "./components/back-to-checkin";

const EventUserPage: React.FC = () => {

    const actionRef = useRef<ActionType>();
    const [open, setOpen] = useState<boolean>(false);
    const id = window.location.href.includes('am') ? '09:00' : '14:30';
    const [openFB, setOpenFB] = useState<boolean>(false);
    const [lead, setLead] = useState<any>();
    const [eventDate, setEventDate] = useState<string>(dayjs().format('YYYY-MM-DD'));
    const access = useAccess();
    const [subLeadOpen, setSubLeadOpen] = useState<boolean>(false);
    const [openHistory, setOpenHistory] = useState<boolean>(false);
    const [openBTC, setOpenBTC] = useState<boolean>(false);
    const { initialState } = useModel('@@initialState');

    const rowClassName = (status: LeadStatus, record: any) => {
        if (!record.salesId) {
            return 'bg-pink-50';
        }
        if (status === LeadStatus.Checkin) {
            return 'bg-green-50';
        }
        if (status === LeadStatus.LeadReject) {
            return 'bg-gray-100';
        }
        if (status === LeadStatus.LeadAccept) {
            return 'bg-blue-50';
        }
        return '';
    }

    return (
        <PageContainer
            extra={(
                <Space>
                    <span>Ngày sự kiện</span>
                    <DatePicker defaultValue={dayjs()} onChange={(date) => {
                        if (date) {
                            setEventDate(date.format('YYYY-MM-DD'));
                            actionRef.current?.reload();
                        }
                    }} />
                </Space>
            )}>
            <div className="overflow-auto">
                <ProTable
                    headerTitle={(
                        <Space>
                            <Button icon={<PlusOutlined />} type="primary" onClick={() => {
                                setLead(null);
                                setOpen(true);
                            }} hidden={!access.event && !access.telesale}>Tạo khách hàng</Button>
                            <TableComponent eventDate={eventDate} eventTime={id} />
                        </Space>
                    )}
                    actionRef={actionRef}
                    request={(params) => apiUsersInEvent({
                        ...params,
                        eventTime: id,
                        eventDate: eventDate
                    })}
                    search={{
                        layout: 'vertical'
                    }}
                    scroll={{
                        x: true
                    }}
                    rowClassName={(record) => rowClassName(record.status, record)}
                    columns={[
                        {
                            title: '#',
                            valueType: 'indexBorder',
                            width: 20
                        },
                        {
                            title: 'Họ và tên',
                            dataIndex: 'name',
                            render: (dom, entity) => (
                                <div>
                                    <div className="font-medium">{entity.gender === true && (<ManOutlined className='text-blue-500' />)}{entity.gender === false && (<WomanOutlined className='text-red-500' />)} {dom}</div>
                                    <div>Đi cùng: {entity.subLeadName}</div>
                                </div>
                            ),
                            minWidth: 150
                        },
                        {
                            title: 'Bàn',
                            dataIndex: 'table',
                            width: 150,
                            search: false,
                            render: (_, entity) => {
                                return (
                                    <div className="text-xs" hidden={!entity.table}>
                                        <div>Bàn: <b>{entity.table}</b></div>
                                        <div>Giờ Check-In: <span className="text-green-900 font-bold">{entity.checkinTime ? entity.checkinTime : '-'}</span></div>
                                        <div>Giờ Check-Out: <span className="text-red-900 font-bold">{entity.checkoutTime ? entity.checkoutTime : '-'}</span></div>
                                    </div>
                                );
                            },
                            minWidth: 80
                        },
                        {
                            title: 'Đi kèm',
                            dataIndex: 'subLeads',
                            render: (dom, entity) => <Button type="primary" icon={<UsergroupAddOutlined className="mr-1" />} disabled={entity.status === LeadStatus.Pending} size="small"
                                onClick={() => {
                                    setLead(entity);
                                    setSubLeadOpen(true);
                                }}>{dom}</Button>,
                            width: 70,
                            align: 'center',
                            search: false
                        },
                        {
                            title: 'Trạng thái',
                            dataIndex: 'tableStatus',
                            search: false,
                            width: 100
                        },
                        {
                            title: 'Người TO',
                            dataIndex: 'to',
                            search: false,
                            width: 100
                        },
                        {
                            title: 'Trợ lý tiếp',
                            dataIndex: 'salesName',
                            search: false,
                            render: (_, entity) => (
                                <div className="min-w-[150px]">{entity.salesName}</div>
                            )
                        },
                        {
                            title: 'Người gọi',
                            dataIndex: 'teleName',
                            search: false,
                            minWidth: 100
                        },
                        {
                            title: 'Thông tin',
                            dataIndex: 'address',
                            search: false,
                            render: (_, entity) => (
                                <div>
                                    <div>Địa chỉ: {entity.adress}</div>
                                    <div>Nghề nghiệp: {entity.jobTitle}</div>
                                </div>
                            )
                        },
                        {
                            title: 'SĐT',
                            dataIndex: 'phoneNumber',
                            width: 80
                        },
                        {
                            title: 'Bước',
                            dataIndex: 'status',
                            valueEnum: {
                                0: <Tag color="yellow">Chờ duyệt</Tag>,
                                1: <Tag color="orange">Đã duyệt</Tag>,
                                2: <Tag color="red">Check-In</Tag>,
                                3: <Tag color="blue">Chốt deal</Tag>,
                                4: <Tag color="black">Từ chối</Tag>,
                                5: <Tag color="green">Hoàn thành</Tag>,
                                6: <Tag color="green">KT Xác nhận</Tag>,
                                7: <Tag color="green">GĐ Xác nhận</Tag>,
                                8: <Tag color="tomato">Mời lại</Tag>
                            },
                            width: 80
                        },
                        {
                            title: 'Miền',
                            dataIndex: 'branch',
                            hideInTable: true,
                            valueType: 'select',
                            fieldProps: {
                                options: BRANCH_OPTIONS
                            }
                        },
                        {
                            title: 'Nguồn',
                            dataIndex: 'source',
                            valueType: 'select',
                            fieldProps: {
                                options: SOURCES
                            },
                            minWidth: 80
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
                                    type="primary" disabled={entity.inviteCount === 1}>{entity.inviteCount}</Button>
                            }
                        },
                        {
                            title: 'Tác vụ',
                            valueType: 'option',
                            render: (_, entity) => [
                                <Popconfirm key="checkin" title="Xác nhận?" onConfirm={async () => {
                                    await apiUpdateLeadStatus({
                                        id: entity.id,
                                        status: LeadStatus.Checkin
                                    });
                                    message.success('Thành công!');
                                    actionRef.current?.reload();
                                }}>
                                    <Tooltip title="Check-in">
                                        <Button type="primary" size="small" icon={<CheckOutlined />}
                                            hidden={(entity.status !== LeadStatus.Approved && entity.status !== LeadStatus.ReInvite) || !access.event}></Button>
                                    </Tooltip>
                                </Popconfirm>,
                                <Popconfirm key="accept" title="Xác nhận?" onConfirm={async () => {
                                    await apiUpdateLeadStatus({
                                        id: entity.id,
                                        status: LeadStatus.LeadAccept
                                    });
                                    message.success('Thành công!');
                                    actionRef.current?.reload();
                                }}>
                                    <Tooltip title="Chốt deal">
                                        <Button size="small" type="primary" icon={<CheckOutlined />} hidden={entity.status !== LeadStatus.Checkin || !access.event}></Button>
                                    </Tooltip>
                                </Popconfirm>,
                                <Popconfirm key="reject" title="Xác nhận?" onConfirm={async () => {
                                    await apiUpdateLeadStatus({
                                        id: entity.id,
                                        status: LeadStatus.LeadReject
                                    });
                                    message.success('Thành công!');
                                    actionRef.current?.reload();
                                }}>
                                    <Tooltip title="Từ chối">
                                        <Button size="small" type="primary" danger
                                            icon={<CloseOutlined />} hidden={entity.status !== LeadStatus.Checkin || !access.event}></Button>
                                    </Tooltip>
                                </Popconfirm>,
                                <Dropdown key="more" menu={{
                                    items: [
                                        {
                                            key: 'copy',
                                            label: 'Sao chép',
                                            icon: <CopyOutlined />,
                                            onClick: () => {
                                                const text = `Họ tên: ${entity.name}.\nĐi cùng: ${entity.subLeadName}.\nSĐT: ${entity.phoneNumber || ''}.\nTelesale: ${entity.teleName || ''}.\nNguồn: ${entity.source || ''}.\nTrạng thái: ${entity.tableStatus || ''}.\nBàn: ${entity.table || ''}.`
                                                navigator.clipboard.writeText(text);
                                                message.success('Sao chép thông tin thành công!');
                                            }
                                        },
                                        {
                                            key: 'edit',
                                            label: 'Chỉnh sửa',
                                            icon: <EditOutlined />,
                                            onClick: () => {
                                                setLead(entity);
                                                setOpen(true);
                                            },
                                            disabled: entity.status === LeadStatus.Pending || !access.event || entity.status === LeadStatus.LeadAccept || entity.status === LeadStatus.LeadReject || entity.status === LeadStatus.Done
                                        },
                                        {
                                            key: 'feedback',
                                            label: 'Feedback',
                                            icon: <CommentOutlined />,
                                            onClick: () => {
                                                setLead(entity);
                                                setOpenFB(true);
                                            },
                                            disabled: entity.status === LeadStatus.Pending || entity.status === LeadStatus.Approved || entity.status === LeadStatus.ReInvite,
                                        },
                                        {
                                            key: 'backToCheckin',
                                            label: 'Chuyển trạng thái về checkin',
                                            icon: <CheckOutlined />,
                                            onClick: () => {
                                                setLead(entity);
                                                setOpenBTC(true);
                                            },
                                            disabled: initialState?.currentUser?.userName !== 'eventhn1' || entity.status !== LeadStatus.LeadReject
                                        }
                                    ]
                                }}>
                                    <Button type="dashed" size="small" icon={<MoreOutlined />} />
                                </Dropdown>
                            ],
                            width: 60
                        }
                    ]}
                />
            </div>

            <LeadForm open={open} onOpenChange={setOpen} lead={lead} eventDate={eventDate} reload={() => {
                actionRef.current?.reload();
            }} />
            <LeadFeedback open={openFB} onOpenChange={setOpenFB}
                eventTime={id}
                id={lead?.id} eventDate={eventDate} reload={() => {
                    actionRef.current?.reload();
                }} />
            <SubLead lead={lead} open={subLeadOpen} onOpenChange={setSubLeadOpen} reload={() => {
                actionRef.current?.reload();
            }} />
            <ReinviteModal open={openHistory} onCancel={() => setOpenHistory(false)} leadId={lead?.id} />
            <BackToCheckinModal open={openBTC} onOpenChange={setOpenBTC} id={lead?.id} reload={() => {
                actionRef.current?.reload();
                setOpenBTC(false);
            }} />
        </PageContainer>
    )
}

export default EventUserPage;